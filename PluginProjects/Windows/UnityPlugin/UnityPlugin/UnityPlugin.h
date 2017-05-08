// 以下の ifdef ブロックは DLL からのエクスポートを容易にするマクロを作成するための 
// 一般的な方法です。この DLL 内のすべてのファイルは、コマンド ラインで定義された UNITYPLUGIN_EXPORTS
// シンボルを使用してコンパイルされます。このシンボルは、この DLL を使用するプロジェクトでは定義できません。
// ソースファイルがこのファイルを含んでいる他のプロジェクトは、 
// UNITYPLUGIN_API 関数を DLL からインポートされたと見なすのに対し、この DLL は、このマクロで定義された
// シンボルをエクスポートされたと見なします。
#ifdef UNITYPLUGIN_EXPORTS
#define UNITYPLUGIN_API __declspec(dllexport)
#else
#define UNITYPLUGIN_API __declspec(dllimport)
#endif

// このクラスは UnityPlugin.dll からエクスポートされました。
class UNITYPLUGIN_API CUnityPlugin {
public:
	CUnityPlugin(void);
	// TODO: メソッドをここに追加してください。
};

extern UNITYPLUGIN_API int nUnityPlugin;

UNITYPLUGIN_API int fnUnityPlugin(void);
